package mocking.microtest.prescriptions

data class Course(var id: String,
                  var name: String,
                  var quantity: String? = null,
                  var dosage: String? = null,
                  var status: CourseStatus? = null)