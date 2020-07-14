package models

import constants.DateTimeFormats
import utils.DateConverter
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import java.time.temporal.ChronoUnit

private const val MONTHS_IN_YEAR = 12
data class PatientAge(val dateOfBirth: String) {

    fun formattedAge(): String {
        val ageMonths: Int = getAgePart(dateOfBirth, ChronoUnit.MONTHS)
        val ageYears: Int = getAgePart(dateOfBirth, ChronoUnit.YEARS)
        var formattedAge = ""
        if (ageYears == 0 && ageMonths == 0) {
            formattedAge = "Less than one month old"
        }
        if (ageYears == 0 && ageMonths == 1) {
            formattedAge = ageMonths.toString() + " month old"
        }
        if (ageYears == 0 && ageMonths > 1) {
            formattedAge = ageMonths.toString() + " months old"
        }
        if (ageYears == 1 && ageMonths >= 0) {
            formattedAge = ageYears.toString() + " year old"
        }
        if (ageYears > 1 && ageMonths >= 0) {
            formattedAge = ageYears.toString() + " years old"
        }
        return formattedAge
    }

    fun formattedDateOfBirth(): String {
        return DateConverter.convertDateToDateTimeFormat(
                dateOfBirth,
                DateTimeFormats.dateWithoutTimeFormat,
                DateTimeFormats.frontendBasicDateFormat)
    }

    fun dateOfBirthDigitsOnly(): String {
        return dateOfBirth.replace("-", "")
    }

    private fun getAgePart(dob: String, unit: ChronoUnit): Int {
        val dateOfBirth = LocalDate.parse(dob, DateTimeFormatter.ISO_DATE)
        return when (unit) {
            ChronoUnit.YEARS -> unit.between(dateOfBirth, LocalDate.now()).toInt()
            ChronoUnit.MONTHS -> (unit.between(dateOfBirth, LocalDate.now()) % MONTHS_IN_YEAR).toInt()
            else -> throw IllegalArgumentException("Only years or months are supported")
        }
    }
}
