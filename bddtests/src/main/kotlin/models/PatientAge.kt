package models

import constants.DateTimeFormats
import utils.DateConverter
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import java.time.temporal.ChronoUnit

private const val MONTHS_IN_YEAR = 12
private const val YEAR_CHAR_LENGTH = 4
data class PatientAge(val dateOfBirth: String) {

    fun formattedAge(): String {
        val ageMonths: Int = getAgePart(dateOfBirth, ChronoUnit.MONTHS)
        val ageYears: Int = getAgePart(dateOfBirth, ChronoUnit.YEARS)
        var formattedAge = ""
        if (ageYears == 0 && ageMonths == 0) {
            formattedAge = "Less than 1 month old"
        }
        else if (ageYears == 0 && ageMonths == 1) {
            formattedAge = "$ageMonths month old"
        }
        else if (ageYears == 1 || (ageYears == 0 && ageMonths > 1)) {
            formattedAge = calculateMonths(ageYears, ageMonths) + " months old"
        }
        else if (ageYears > 1) {
            formattedAge = "$ageYears years old"
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

    fun getYearOfBirth(): String{
        return dateOfBirth.substring(0, YEAR_CHAR_LENGTH)
    }

    private fun getAgePart(dob: String, unit: ChronoUnit): Int {
        val dateOfBirth = LocalDate.parse(dob, DateTimeFormatter.ISO_DATE)
        return when (unit) {
            ChronoUnit.YEARS -> unit.between(dateOfBirth, LocalDate.now()).toInt()
            ChronoUnit.MONTHS -> (unit.between(dateOfBirth, LocalDate.now()) % MONTHS_IN_YEAR).toInt()
            else -> throw IllegalArgumentException("Only years or months are supported")
        }
    }

    private fun calculateMonths(years: Int, months: Int) : String {
        return ((years * MONTHS_IN_YEAR) + months).toString()
    }
}
