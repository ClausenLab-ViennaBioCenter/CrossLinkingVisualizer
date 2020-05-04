Imports CrossLinkingVisualizer

Public Interface ICrosslinkVisualizationProviderAsync

    ReadOnly Property UpdateAsync(visualizerViewModel As VisualizerViewModel) As Task

End Interface

Module InterfaceExtensions

    ''' <summary>
    ''' Ensure the async versions always have a synchronous implementation as well if neccessary
    ''' </summary>
    ''' <param name="referenceObject"></param>
    ''' <param name="inputModel"></param>
    <System.Runtime.CompilerServices.Extension>
    Public Sub UpdateSynchronously(referenceObject As ICrosslinkVisualizationProviderAsync, inputModel As VisualizerViewModel)
        referenceObject.UpdateAsync(inputModel).GetAwaiter().GetResult()
    End Sub


End Module

