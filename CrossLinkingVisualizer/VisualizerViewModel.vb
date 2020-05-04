Imports VBExtensions.IOExtensions
Imports VBExtensions
Imports PymolInterface

Public Class VisualizerViewModel
    Implements ComponentModel.INotifyPropertyChanged

    Private _FDRHardLimit As Double
    Public Event PropertyChanged As ComponentModel.PropertyChangedEventHandler Implements ComponentModel.INotifyPropertyChanged.PropertyChanged

    Public ReadOnly Property Data As CrosslinkingData
    Public Property Parent As MainWindow
    Public Property Painter As CrossLinkingPainter

    Public Property VisualizationProviders As List(Of ICrosslinkVisualizationProviderAsync)

    Public Property FDRHardLimit As Double
        Get
            Return _FDRHardLimit
        End Get
        Set
            _FDRHardLimit = Value
            RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs(NameOf(FDRHardLimit)))
        End Set
    End Property

    Public ReadOnly Property Command_ShowWindow As ICommand
    Public ReadOnly Property Command_ImportTemplate As ICommand
    Public ReadOnly Property Command_ImportCrosslinks As ICommand
    Public ReadOnly Property Command_ImportAnnotation As ICommand
    Public ReadOnly Property Command_ImportSecondaryStructure As ICommand
    Public ReadOnly Property Command_UpdateVisualizations As ICommand
    Public ReadOnly Property Command_UpdatePymolPath As ICommand
    Public ReadOnly Property Command_ExportSVG As ICommand
    Public ReadOnly Property Command_Color3DBySecondaryStructure As ICommand

    Private Function ImportCrossLinksAllowed() As Boolean
        Return True
    End Function

    Private Function ImportTemplateAllowed() As Boolean
        Return True
    End Function

    Private Sub ImportTemplate()

        Dim File As ExistingFilePath

        Dim TargetName As String
        Dim SequenceString As String
        Dim StartIndex As Integer
        Dim EndIndex As Integer

#If DEBUG Then

        If IsNothing(Data.SourceObjects) OrElse Data.SourceObjects.Count = 0 Then
            File = New ExistingFilePath("\\storage.imp.ac.at\groups\clausen\_General Scripts Tools\new\CrossLinkingVisualizer\example_inputs\example_template_Mys1b.txt")
        Else
            Try
                File = FileImporter.SelectFile
            Catch ex As Exception
                MsgBox("Error importing file, " & ex.Message)
                Exit Sub
            End Try

            If File Is Nothing Then Exit Sub
        End If

#Else
               Try
            File = FileImporter.SelectFile()
        Catch ex As Exception
            MsgBox("Error importing file, " & ex.Message)
            Exit Sub
        End Try

        If File Is Nothing Then Exit Sub
#End If

        Using reader As New System.IO.StreamReader(File.FilePath)
            With reader

                TargetName = .ReadLine()

                Dim Indices = .ReadLine().Split(",")

                StartIndex = Indices.First()
                EndIndex = Indices.Last()

                'TODO: this is a hacky support for fasta
                If TargetName.First = ">" Then TargetName = TargetName.Substring(1)

                SequenceString = .ReadLine().ToUpper() '.ToCharArray().Where(Function(x) Not Char.IsWhiteSpace(x)).ToArray().ToString()
                'TODO: REMOVE WHITESPACE

            End With
        End Using

        If Data.SourceObjects.Where(Function(x) String.Equals(TargetName, x.Name)).Count > 0 Then
            Throw New ArgumentException("Template with the given name is already imported")
        End If

        With SequenceString

            Dim AllowedCharacters As String = "ACDEFGHIKLMNPQRSTVWXY"

            If Not SequenceString.ToCharArray().All(Function(x) AllowedCharacters.Contains(x)) Then
                Throw New ArgumentOutOfRangeException("Non protein coding characters found")
            End If

            Dim SequenceLength As Integer = 1 + EndIndex - StartIndex

            If .Length < SequenceLength Then
                .PadRight(SequenceLength, "X")
            End If

            Select Case True
                Case .Length < SequenceLength
                    .PadRight(SequenceLength, " ")
                Case .Length > SequenceLength
                    EndIndex = StartIndex + .Length - 1
                Case Else
                    'All fine, annotation lengths match
            End Select

        End With

        Dim NewCrosslinkingTemplateObject = New CrosslinkingSourceObjectViewModel(TargetName, SequenceString, StartIndex, EndIndex, Painter)

        Data.SourceObjects.Add(NewCrosslinkingTemplateObject)
        Painter.DrawTemplate(NewCrosslinkingTemplateObject)

    End Sub

    Private Function DrawAllowed() As Boolean
        Return True
    End Function

    Public Sub ShowWindow()

        If Painter Is Nothing Then
            Painter = New CrossLinkingPainter(Me)
            For Each sourceObj In Data.SourceObjects
                sourceObj.Parent = Painter
            Next
        End If

        Painter.ShowWindow()

    End Sub



    Public Sub New()

        Data = New CrosslinkingData()

        VisualizationProviders = New List(Of ICrosslinkVisualizationProviderAsync)

        FDRHardLimit = 0.2

        Command_UpdateVisualizations = New DelegateCommand(Of String)(AddressOf UpdateVisualizationProviders, AddressOf UpdateVisualizationProvidersAllowed)
        Command_ImportTemplate = New DelegateCommand(Of String)(AddressOf ImportTemplate, AddressOf ImportTemplateAllowed, AddressOf ImportExceptionHandler)
        Command_ImportSecondaryStructure = New DelegateCommand(Of String)(AddressOf ImportSecondaryStructure)
        Command_ImportAnnotation = New DelegateCommand(Of String)(AddressOf ImportDomains, AddressOf ImportDomainsAllowed, AddressOf ImportDomainsExceptionHandler)
        Command_ImportCrosslinks = New DelegateCommand(Of String)(AddressOf ImportCrosslinkData, AddressOf ImportCrosslinkDataAllowed, AddressOf ImportCrosslinkDataExceptionHandler)
        Command_UpdatePymolPath = New DelegateCommand(Of String)(AddressOf UpdatePymolPath)
        Command_ExportSVG = New DelegateCommand(Of String)(AddressOf ExportSVG)
        Command_Color3DBySecondaryStructure = New DelegateCommand(Of String)(AddressOf Color3DBySecondaryStructure)

        Command_ShowWindow = New DelegateCommand(Of String)(AddressOf ShowWindow, AddressOf DrawAllowed)

        'Bad idea to draw on new, because of how WPF treats DataContext in XAML - it automatically instantiates it, which gives problems
        'Both during design and later
        'Draw()


    End Sub

    Private Sub Color3DBySecondaryStructure(obj As String)

        Dim pymolProvider As Pymol3DVisualizationProvider = VisualizationProviders.Where(Function(x) TypeOf x Is Pymol3DVisualizationProvider).Single()

        pymolProvider.ColorBySS

    End Sub

    Private Sub ExportSVG(obj As String)

        Dim Dialog As New Microsoft.Win32.SaveFileDialog

        Dialog.ShowDialog()

        Dim TargetFile = Dialog.FileName

        IO.File.WriteAllText(TargetFile, Painter.GenerateSVG)

    End Sub

    Private Sub UpdatePymolPath(obj As String)

        PymolController.UpdatePymolPath

    End Sub

    Private Function UpdateVisualizationProvidersAllowed(obj As Exception) As Boolean
        Return True
    End Function

    Private Async Sub UpdateVisualizationProviders(obj As String)

        For Each provider In VisualizationProviders

            provider.UpdateAsync(Me).GetAwaiter().GetResult()

            'Task.Run(Sub() provider.UpdateAsync(Me).ConfigureAwait(False))

        Next

    End Sub


    Private Sub CommandSharedExceptionHandler(obj As Exception)
        With obj

            If .GetType() Is GetType(ArgumentException) AndAlso .InnerException Is Nothing Then
                MsgBox(obj.Message)
            Else
                Throw New Exception(String.Format("Unhandled exception coming from {0}: {1}", .Source, .Message), obj)
            End If

        End With
    End Sub

    Private Sub ImportCrosslinkDataExceptionHandler(obj As Exception)
        CommandSharedExceptionHandler(obj)
    End Sub

    Private Sub ImportDomainsExceptionHandler(obj As Exception)
        CommandSharedExceptionHandler(obj)
    End Sub

    Private Sub ImportExceptionHandler(obj As Exception)
        CommandSharedExceptionHandler(obj)
    End Sub

    Private Function ImportDomainsAllowed(obj As String) As Boolean
        Return True
    End Function

    Private Sub ImportDomains(obj As String)
        Throw New NotImplementedException
    End Sub

    Private Sub ImportSecondaryStructure(obj As String)

        Dim File As ExistingFilePath
        Dim TargetName As String
        Dim AnnotationString As String
        Dim AnnotationArray As Char()
        Dim StartIndex As Integer
        Dim EndIndex As Integer

        Dim referenceTemplateObjectViewModel As CrosslinkingSourceObjectViewModel



#If DEBUG Then
        If Data.SourceObjects.First().Annotations.SecondaryStructure.Count = 0 Then
            File = New ExistingFilePath("\\storage.imp.ac.at\groups\clausen\_General Scripts Tools\new\CrossLinkingVisualizer\example_inputs\example_secondaryStructure_Mys1b_psipred.txt")
        Else
            File = FileImporter.SelectFile
        End If
#Else
        File = FileImporter.SelectFile()
#End If

        'TODO: pre-check if it's empty, no need to read the file if yes
        'If ExistingTemplateNames.Count= 0 Then

        Using reader As New System.IO.StreamReader(File.FilePath)
            With reader

                TargetName = .ReadLine()

                Dim Indices = .ReadLine().Split(",")

                StartIndex = Indices.First()
                EndIndex = Indices.Last()

                AnnotationString = .ReadLine().ToUpper()

            End With
        End Using

        If Data.SourceObjects.Where(Function(x) String.Equals(TargetName, x.Name)).Count = 0 Then
            Throw New ArgumentException(String.Format("Target template for the SS annotation does not exist, import it first ({0})", TargetName))
        End If

        referenceTemplateObjectViewModel = Data.SourceObjects.Where(Function(x) x.Name = TargetName).Single()

        Dim AnnotationLength As Integer = 1 + EndIndex - StartIndex

        Select Case True
            Case AnnotationString.Length < AnnotationLength
                AnnotationString.PadRight(AnnotationLength, " ")
            Case AnnotationString.Length > AnnotationLength
                EndIndex = StartIndex + AnnotationString.Length - 1
            Case Else
                'All fine, annotation lengths match
        End Select

        If StartIndex < referenceTemplateObjectViewModel.StartIndex OrElse EndIndex > referenceTemplateObjectViewModel.EndIndex Then
            Throw New ArgumentOutOfRangeException(
                "The annotations refer to locations not present in the referenced template (e.g. index of a helix is larger than length of the underlying sequence")
        End If

        AnnotationArray = AnnotationString.ToCharArray()

        Dim SSCollection As New List(Of SecondaryStructureViewModel)

        Dim CurrentChar As Char = AnnotationArray.First()
        Dim CurrentSS As SecondaryStructureViewModel

        If CurrentChar <> "C" Then
            CurrentSS = New SecondaryStructureViewModel(CurrentChar)
            CurrentSS.StartIndex = 1
        End If

        For i = 1 To AnnotationArray.Length - 1

            If Not CurrentChar.In("E", "H", "C", "-", " ") Then
                Throw New ArgumentOutOfRangeException("Invalid or unsuppored secondary structure annotation.")
            End If

            If AnnotationArray(i) <> CurrentChar Then

                CurrentChar = AnnotationArray(i)

                If CurrentSS IsNot Nothing Then
                    CurrentSS.EndIndex = i
                    SSCollection.Add(CurrentSS)
                End If

                Select Case CurrentChar
                    Case "E", "H"
                        CurrentSS = New SecondaryStructureViewModel(CurrentChar)
                        CurrentSS.StartIndex = i + 1
                    Case "C", " ", "-"
                        CurrentSS = Nothing
                    Case Else
                        Throw New ArgumentOutOfRangeException("Invalid or unsuppored secondary structure annotation.")
                End Select

            End If

        Next

        If CurrentSS IsNot Nothing Then
            CurrentSS.EndIndex = AnnotationArray.Length
        End If

        referenceTemplateObjectViewModel.Annotations.SecondaryStructure = SSCollection

        Painter.DrawSS()

    End Sub


    Private Function ImportCrosslinkDataAllowed(obj As String) As Boolean
        Return True
    End Function

    Private Sub ImportCrosslinkData()

        Dim File As ExistingFilePath

        Dim RequiredTemplateNames As IEnumerable(Of String)
        Dim ExistingTemplateNames As IEnumerable(Of String)


#If DEBUG Then

        If IsNothing(Data.Crosslinks) OrElse Data.Crosslinks.Count = 0 Then
            File = New ExistingFilePath("\\storage.imp.ac.at\groups\clausen\_General Scripts Tools\new\CrossLinkingVisualizer\example_inputs\example_crosslinks_Mys1b.csv")
            'File = New ExistingFilePath("\\storage.imp.ac.at\groups\clausen\_General Scripts Tools\new\CrossLinkingVisualizer\example_inputs\example_crosslinks_Mys1b_test2.csv")
        Else
            File = FileImporter.SelectFile
        End If

        'File = FileImporter.SelectFile(iMultiselect:=False)
#Else

        File = FileImporter.SelectFile()

#End If


        If File Is Nothing Then Exit Sub

        ExistingTemplateNames = Data.SourceObjects.Select(Function(x) x.Name)

        'TODO: pre-check if it's empty, no need to read the file if yes
        'If ExistingTemplateNames.Count= 0 Then 

        Dim Lines As New List(Of String())

        With New Microsoft.VisualBasic.FileIO.TextFieldParser(File.FilePath)
            .TextFieldType = FileIO.FieldType.Delimited
            .SetDelimiters(",")
            While Not .EndOfData
                Lines.Add(.ReadFields())
            End While
        End With

        RequiredTemplateNames = Lines.Select(Function(x) x(0)).
            Concat(Lines.Select(Function(x) x(1))).
            Distinct().Except(ExistingTemplateNames)

        With RequiredTemplateNames
            If .Count() > 0 Then

                Throw New ArgumentException(String.Format("The following entries are referenced by the crosslinks but don't exist: {0}",
                                        .Aggregate(Function(x, y) x & ", " & y)))

            End If
        End With

        'TODO: include header? and then Lines.Skip(1)
        For Each CurrentEntries As String() In Lines

            Dim SourceObj As String = CurrentEntries(0)
            Dim TargetObj As String = CurrentEntries(1)
            Dim SourceIndex As Integer = CInt(CurrentEntries(2))
            Dim TargetIndex As Integer = CInt(CurrentEntries(3))

            'TODO: NOOOOOOOOOOOOO

            Dim FDR As Double = 0
            Dim PSMC As Integer = 0
            Dim PepPairC As Integer = 0

            If CurrentEntries.Count() >= 6 Then
                'TODO: check whether this works both with comma and dot
                FDR = Double.Parse(CurrentEntries(4), Globalization.CultureInfo.InvariantCulture)
                PSMC = CInt(CurrentEntries(5))
                PepPairC = CInt(CurrentEntries(6))
            End If

            'Skip entries worse than hard limit
            If FDR > FDRHardLimit Then Continue For

            If SourceObj = TargetObj AndAlso TargetIndex < SourceIndex Then
                SwapValue(SourceIndex, TargetIndex)
            End If

            With Data.SourceObjects

                Dim SourceObjRef As CrosslinkingSourceObjectViewModel = .Where(Function(x) x.Name = SourceObj).Single()
                Dim TargetObjRef As CrosslinkingSourceObjectViewModel = .Where(Function(x) x.Name = TargetObj).Single()

                If Not SourceObjRef.ValidateIndex(SourceIndex) Then Throw New Exception()
                If Not TargetObjRef.ValidateIndex(TargetIndex) Then Throw New Exception()

                Dim NewCrosslink = New CrosslinkViewModel(SourceObjRef,
                                                       TargetObjRef,
                                                       SourceIndex,
                                                       TargetIndex,
                                                       File.FilePath)

                With NewCrosslink
                    .FDR = FDR
                    .PSMCount = PSMC
                    .PeptidePairCount = PepPairC
                End With

                Data.Crosslinks.Add(NewCrosslink)

            End With

        Next

        Painter.DrawCrosslinks()

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class