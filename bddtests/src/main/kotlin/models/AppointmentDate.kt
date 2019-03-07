package models
import java.time.LocalDateTime

data class AppointmentDate (var date: LocalDateTime,
                            var hour: Int,
                            var minute: Int,
                            var duration: Int = 10,
                            var sessionName: String)