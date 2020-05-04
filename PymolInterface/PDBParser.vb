Public Structure PDBRecord
    Public RecordType as String
    Public AtomSerialNumber as Integer
    Public AtomName      as string
    Public AlternativeLocationIndicator as string
    Public ResidueName as string
    Public ChainID   as string
    Public ResidueSequenceNumber    as Integer
    Public InsertionCode     as String
    Public CoordinateX         as double
    Public CoordinateY         as double
    Public CoordinateZ         as double
    Public Occupancy as double
    Public TempFactor as double
    Public Element   as string
    Public Charge as String
End Structure

Public Class PDBRecordString
    Public RecordTypeString(5) as Char
    Public AtomSerialNumberString(4) as Char
    Public AtomNameString(3) as Char
    Public AlternativeLocationIndicatorString(0) as Char
    Public ResidueNameString(2) as Char
    Public ChainIDString(0) as Char
    Public ResidueSequenceNumberString(3) as Char
    Public InsertionCodeString(0) as Char
    Public CoordinateXString(7) as Char
    Public CoordinateYString(7) as Char
    Public CoordinateZString(7) as Char
    Public OccupancyString(5) as Char
    Public TempFactorString(5) as Char
    Public ElementString(1) as Char
    Public ChargeString(1) as Char
End Class

Public Class PDBParser

    Public Sub LoadFromText(textFile As String)

        Dim FileContents=IO.File.ReadAllLines(textFile)

        
        





    End Sub



End Class
