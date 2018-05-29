package mocking.emis.models

enum class PrescriptionType(val prescriptionType: Int)
{
    Unknown(1),
    Acute(2),
    Repeat(3),
    RepeatDispensing(4),
    Automatic(5)
}
