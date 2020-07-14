import java.util.*

fun Calendar.addDays(days: Int): Calendar {
    this.add(Calendar.DATE, days)
    return this
}

fun Calendar.addHours(hour: Int): Calendar {
    this.add(Calendar.HOUR, hour)
    return this
}

fun Calendar.addMinutes(minutes: Int): Calendar {
    this.add(Calendar.MINUTE, minutes)
    return this
}

fun Calendar.reset(now: Date): Calendar {
    this.time = now
    return this
}
