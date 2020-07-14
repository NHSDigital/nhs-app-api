package models
import java.time.LocalDateTime

data class AppointmentDate (var date: LocalDateTime,
                            var hour: Int? = null,
                            var minute: Int? = null,
                            var duration: Int = 10)
