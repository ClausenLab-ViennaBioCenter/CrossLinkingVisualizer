Imports CrossLinkingVisualizer
Imports PymolInterface

Public Class Pymol3DVisualizationProvider
    Implements ICrosslinkVisualizationProviderAsync

    Private ReadOnly Property sourceViewModel As VisualizerViewModel
    Private Property PymolController As PymolController
    Private ReadOnly Property ModelViewModels As List(Of Crosslinking3DVisualizationViewModel)

    Private Property ModelsValid As Boolean



    Private ReadOnly Property MolecularModels As List(Of MolecularModelViewModel)
        Get
            Return ModelViewModels.Select(Function(x) x.MolecularModels).SelectMany(Function(x) x).ToList()
        End Get
    End Property

    Public ReadOnly Property UpdateAsync(visualizerViewModel As VisualizerViewModel) As Task Implements ICrosslinkVisualizationProviderAsync.UpdateAsync
        Get
            Return UpdateFromSource()
        End Get
    End Property

    Public Sub New(iSourceViewModel As VisualizerViewModel)

        sourceViewModel = iSourceViewModel
        PymolController = New PymolController()
        ModelViewModels = New List(Of Crosslinking3DVisualizationViewModel)()

        AddHandler sourceViewModel.Data.PropertyChanged, AddressOf OnSourceDataChanged

    End Sub

    Private Sub OnSourceDataChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs)

        If sender Is sourceViewModel.Data Then
            Select Case e.PropertyName
                Case NameOf(sourceViewModel.Data.SourceObjects)
                    InvalidateModels()
                Case NameOf(sourceViewModel.Data.Crosslinks)

                Case Else
                    Throw New NotImplementedException
            End Select
        End If

    End Sub

    Private Sub InvalidateModels()
        ModelsValid = False
    End Sub

    Private Async Function UpdateFromSource() As Task

        Await UpdateModels()

        Await EnsurePymolIsAvailable()

        Dim Commands As List(Of String) = Await GeneratePymolCommands()


        PymolController.InputStream.WriteLineAsync(String.Join(Environment.NewLine, Commands))


        'Dim ModelDictionary As New Dictionary(Of CrosslinkingSourceObjectViewModel, List(Of MolecularModelViewModel))
        'ModelViewModels.ForEach(Sub(x)
        '                            ModelDictionary.Add(x.SourceCrosslinkingObject, x.MolecularModels.ToList())
        '                        End Sub)

        'Dim modelSelectionStringsDictionary As Dictionary(Of CrosslinkingSourceObjectViewModel, String) = PreCreateSelectionStrings()


        '.WriteLine("delete *")

        'For Each model In MolecularModels
        '    .WriteLine(String.Format("load {0}, {1}", model.Filename.FilePath, model.ModelName))
        '    Threading.Thread.Sleep(50)
        'Next

        '.Flush()

        'For Each crosslink In sourceViewModel.Data.Crosslinks.Where(Function(x) x.ShouldBeVisible)

        '    Dim startIndex = crosslink.SourceLocus
        '    Dim endIndex = crosslink.TargetLocus

        '    Dim name As String = String.Format("d_{0}_{1}", startIndex, endIndex)

        '    'skip templates with no 3D model
        '    If Not ModelDictionary.ContainsKey(crosslink.SourceObjectViewModel) Then Continue For
        '    If Not ModelDictionary.ContainsKey(crosslink.TargetObjectViewModel) Then Continue For

        '    Dim modelSelectionSource As String = modelSelectionStringsDictionary(crosslink.SourceObjectViewModel)
        '    Dim modelSelectionTarget As String = modelSelectionStringsDictionary(crosslink.TargetObjectViewModel)

        '    Dim distanceCommand As String = String.Format("dist {0}, ({3}) and resi {1} and n. ca, ({4}) and resi {2} and n. ca",
        '                                                  name, startIndex, endIndex, modelSelectionSource, modelSelectionTarget)

        '    .WriteLine(distanceCommand)

        'Next

        ''.WriteLine("dist d1, resi 30 and n. ca, resi 50 and n. ca")
        '.Flush()

        '.WriteLine("group distances, d_*_*")
        '.WriteLine("reset")

        'End With


    End Function

    Private Async Function GeneratePymolCommands() As Task(Of List(Of String))


        Dim ModelDictionary As New Dictionary(Of CrosslinkingSourceObjectViewModel, List(Of MolecularModelViewModel))
        ModelViewModels.ForEach(Sub(x)
                                    ModelDictionary.Add(x.SourceCrosslinkingObject, x.MolecularModels.ToList())
                                End Sub)

        Dim modelSelectionStringsDictionary As Dictionary(Of CrosslinkingSourceObjectViewModel, String) = PreCreateSelectionStrings()

        Dim tempOut As New List(Of String)

        With tempOut

            .Capacity = 5000
            .Add("delete *")

            For Each model In MolecularModels
                .Add(String.Format("load {0}, {1}", model.Filename, model.ModelName))
            Next

            For Each crosslink In sourceViewModel.Data.Crosslinks.Where(Function(x) x.ShouldBeVisible)

                Dim startIndex = crosslink.SourceLocus
                Dim endIndex = crosslink.TargetLocus

                Dim name As String = String.Format("d_{0}_{1}", startIndex, endIndex)

                'skip templates with no 3D model
                If Not ModelDictionary.ContainsKey(crosslink.SourceObjectViewModel) Then Continue For
                If Not ModelDictionary.ContainsKey(crosslink.TargetObjectViewModel) Then Continue For

                Dim modelSelectionSource As String = modelSelectionStringsDictionary(crosslink.SourceObjectViewModel)
                Dim modelSelectionTarget As String = modelSelectionStringsDictionary(crosslink.TargetObjectViewModel)

                Dim distanceCommand As String = String.Format("dist {0}, ({3}) and resi {1} and n. ca, ({4}) and resi {2} and n. ca",
                                                              name, startIndex, endIndex, modelSelectionSource, modelSelectionTarget)

                .Add(distanceCommand)

            Next

            .add("group distances, d_*_*")
            .Add("reset")

        End With

        Return tempOut

    End Function

    Private Function PreCreateSelectionStrings() As Dictionary(Of CrosslinkingSourceObjectViewModel, String)

        Const pymolJoinOperator As String = " or "

        Dim tDict As New Dictionary(Of CrosslinkingSourceObjectViewModel, String)

        ModelViewModels.ForEach(Sub(x)
                                    tDict.Add(x.SourceCrosslinkingObject,
                                              String.Join(pymolJoinOperator, x.MolecularModels.Select(Function(y) y.ModelName)))
                                End Sub)

        Return tDict


    End Function

    Private Async Function UpdateModels() As Task

        If ModelsValid Then

            Exit Function

        Else

            Dim tempVM As New ModelVizualization3DInputWindowViewModel(sourceViewModel.Data.SourceObjects.ToList(), ModelViewModels.ToList())
            Dim ModelInputWindow As New ModelVizualization3DInputWindow(tempVM)

            With ModelInputWindow
                .ShowDialog()
            End With

            ModelViewModels.Clear()
            ModelViewModels.AddRange(tempVM.VisualizationViewModels)

            ModelsValid = True

        End If

    End Function

    Private Async Function EnsurePymolIsAvailable() As Task

        If PymolController Is Nothing Then PymolController = New PymolController()

        'TODO: add check whether pymol process is actually running
        If PymolController.Process.HasExited Then
            PymolController = New PymolController()
        End If

    End Function


    Public Async Sub ColorBySS()

        MsgBox("For now this is only correct if there is only one protein. This is only a test.")

        Await EnsurePymolIsAvailable()

        If Not ModelsValid Then

            MsgBox("you have to import models and draw stuff first")
            Exit Sub

        Else

            Dim Strings As New List(Of String)

            Strings.Add("color grey")

            For Each ss In sourceViewModel.Data.SourceObjects.First().Annotations.SecondaryStructure

                Select Case ss.SSType
                    Case "H"
                        Strings.Add(String.Format("color red, resi {0}-{1}", ss.StartIndex, ss.EndIndex))
                    Case "E"
                        Strings.Add(String.Format("color green, resi {0}-{1}", ss.StartIndex, ss.EndIndex))
                End Select

            Next

            PymolController.InputStream.WriteLine(String.Join(Environment.NewLine, Strings))

        End If

    End Sub
End Class
