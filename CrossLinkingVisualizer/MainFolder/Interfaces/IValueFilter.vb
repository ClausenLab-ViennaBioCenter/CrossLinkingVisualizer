Imports System.ComponentModel

Public Interface IValueFilter
    Inherits INotifyPropertyChanged
    Property HighLimit() As Double
    Property LowLimit() As Double
    Function CheckFilter(crosslinkViewModel As CrosslinkViewModel) As Boolean
End Interface


