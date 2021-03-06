﻿Class MainWindow

    Public ReadOnly Property ProgramNameAndVersion() As String
        Get
            Dim name = Reflection.Assembly.GetEntryAssembly().GetName().Name
            Dim version = FileVersionInfo.GetVersionInfo(Reflection.Assembly.GetEntryAssembly().Location).FileVersion
            'Return name.FileName & vbCrLf & String.Format("{0}.{1}.{2}.{3}", name.Version.Major, name.Version.Minor, name.Version.Build, name.Version.MajorRevision)
            Return name & " " & String.Format("{0}", version)
        End Get
    End Property

    Public ReadOnly Property ApplicationIcon As String
        Get
            'TODO: make dynamic
            Return "CrossLinkVisualizer_Icon_v1.ico"
        End Get
    End Property

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        AddHandler Application.Current.DispatcherUnhandledException, Sub(o as Object, e as System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)
                                                                        e.Handled=True
                                                                        MsgBox("An unhandled error has occured: " & e.Exception.Message)
                                                                     End Sub


        Dim VM = New VisualizerViewModel

        VM.Parent = Me

        VM.VisualizationProviders.Add(New Pymol3DVisualizationProvider(VM))

        '#If DEBUG Then
        '        With MolecularModels
        '            .Add("\\storage.imp.ac.at\groups\clausen\tim\rnf213\juraj\sa.pdb")
        '            .Add("\\storage.imp.ac.at\groups\clausen\tim\rnf213\juraj\sb.pdb")
        '            .Add("\\storage.imp.ac.at\groups\clausen\tim\rnf213\juraj\sc.pdb")
        '            .Add("\\storage.imp.ac.at\groups\clausen\tim\rnf213\juraj\mist.pdb")
        '            .Add("\\storage.imp.ac.at\groups\clausen\tim\rnf213\juraj\sd.pdb")
        '            .Add("\\storage.imp.ac.at\groups\clausen\tim\rnf213\juraj\se.pdb")
        '        End With
        '#End If

        Me.DataContext = VM

        VM.ShowWindow()

    End Sub


End Class
