Imports System.Math

Public Module MathExtensions
    '****************************************************************************************************
    Public Function RangeOverlaps(
    ByVal ProbeStart As Int32,
    ByVal ProbeEnd As Int32,
    ByVal RangeStart As Int32,
    ByVal RangeEnd As Int32
    ) As Boolean

        '====================================================================================================
        'Tests if two numeric ranges overlap
        'Juraj Ahel, 2016-12-20, for Gibson assembly

        '====================================================================================================
        'This should be overloaded for each data type...

        If ((ProbeStart < RangeStart) And (ProbeEnd < RangeStart)) Or ((ProbeStart > RangeEnd) And (ProbeEnd > RangeEnd)) Then
            RangeOverlaps = False
        Else
            RangeOverlaps = True
        End If

    End Function



    '****************************************************************************************************
    Public Function RoundToSignificantDigits(
    ByVal NumberToRound As Double,
    ByVal SignificantDigits As Int32
    ) As Double

        '====================================================================================================
        'Rounds input X to Y significant digits
        '
        'Juraj Ahel, 2017-02-07, for general purposes
        '====================================================================================================

        If NumberToRound = 0 Then
            RoundToSignificantDigits = 0
        Else
            RoundToSignificantDigits = Round(NumberToRound, SignificantDigits - CInt(Int(Lg(NumberToRound) + 1)))
        End If

    End Function

    Public Enum RoundingType
        RoundClosest
        RoundUp
        RoundDown
    End Enum

    '****************************************************************************************************
    Public Function RoundToNearestX(
                                    NumberToRound As Double,
                                    RoundingFactor As Double,
                                    Optional RoundType As RoundingType = RoundingType.RoundClosest) As Double

        Select Case RoundType

            Case RoundingType.RoundClosest
                RoundToNearestX = RoundingFactor * Round(NumberToRound / RoundingFactor)

            Case RoundingType.RoundUp
                'if an exact multiple, don't add one
                If Int(NumberToRound / RoundingFactor) = NumberToRound / RoundingFactor Then
                    RoundToNearestX = RoundingFactor * Int(NumberToRound / RoundingFactor)
                Else
                    RoundToNearestX = RoundingFactor * (1 + Int(NumberToRound / RoundingFactor))
                End If


            Case RoundingType.RoundDown
                RoundToNearestX = RoundingFactor * Int(NumberToRound / RoundingFactor)
        End Select

    End Function

    '****************************************************************************************************
    Public Function RoundToNearestX(
    ByVal NumberToRound As Double,
    ByVal RoundingFactor As Double,
    Optional ByVal RoundDown As Boolean = False) As Double

        '====================================================================================================
        'Rounds input X to the nearest multiple of input Y
        '
        'Juraj Ahel, 2015-04-23, for general purposes
        'Last update 2015-04-23
        '2016-06-13 add RoundDown Flag
        '====================================================================================================

        If RoundDown Then
            RoundToNearestX = RoundingFactor * Int(NumberToRound / RoundingFactor)
        Else
            RoundToNearestX = RoundingFactor * Round(NumberToRound / RoundingFactor)
        End If

    End Function



    '****************************************************************************************************
    Public Function Lg(ByVal a As Double) As Double
        '====================================================================================================
        'Logarithm base 10
        'Juraj Ahel, 2015-02-11
        'Last update 2015-02-11
        '====================================================================================================

        Lg = Log(a) / Log(10)

    End Function
    '****************************************************************************************************
    Public Function Ln(ByVal a As Double) As Double
        '====================================================================================================
        'Logarithm base e (natural logarithm)
        'Juraj Ahel, 2015-02-11
        'Last update 2015-02-11
        '====================================================================================================

        Ln = Log(a)

    End Function
    '****************************************************************************************************
    Public Function Lb(ByVal a As Double) As Double
        '====================================================================================================
        'Logarithm base 2
        'Juraj Ahel, 2015-02-11
        'Last update 2015-02-11
        '====================================================================================================

        Lb = Log(a) / Log(2)

    End Function

    '****************************************************************************************************
    Public Function FMod(ByVal dividend As Double, ByVal divisor As Double) As Double

        FMod = dividend - Fix(dividend / divisor) * divisor

        'http://en.wikipedia.org/wiki/Machine_epsilon
        'Unfortunately, this function can only be accurate when `dividend / divisor` is outside [-2.22E-16,+2.22E-16]
        'Without this correction, FMod(.66, .06) = 5.55111512312578E-17 when it should be 0
        'TODO: check if values make sense
        If FMod >= -2 ^ -52 And FMod <= 2 ^ -52 Then '+/- 2.22E-16
            FMod = 0
        End If

    End Function

    '****************************************************************************************************
    Public Function FMod(ByVal dividend As Single, ByVal divisor As Single) As Single

        FMod = dividend - Fix(dividend / divisor) * divisor

        'http://en.wikipedia.org/wiki/Machine_epsilon
        'Unfortunately, this function can only be accurate when `dividend / divisor` is outside [-2.22E-16,+2.22E-16]
        'Without this correction, FMod(.66, .06) = 5.55111512312578E-17 when it should be 0
        'TODO: check if values make sense
        If FMod >= -2 ^ -52 And FMod <= 2 ^ -52 Then '+/- 2.22E-16
            FMod = 0
        End If

    End Function
End Module