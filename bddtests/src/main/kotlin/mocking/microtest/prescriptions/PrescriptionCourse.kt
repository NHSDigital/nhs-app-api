package mocking.microtest.prescriptions

data class PrescriptionCourse(var id: String,
                              var orderDate: String,
                              var status: PrescriptionStatus,
                              var name: String,
                              var quantity: String,
                              var dosage: String,
                              var type: String,
                              var reason: String
                              )
