Public Class CrossLinkingObjectAnnotations
    Public Property SecondaryStructure As List(Of SecondaryStructureViewModel)
    Public Property Domains As List(Of DomainViewModel)

    Public Sub New()
        SecondaryStructure=New List(Of SecondaryStructureViewModel)
        Domains=New List(Of DomainViewModel)
    End Sub

End Class
