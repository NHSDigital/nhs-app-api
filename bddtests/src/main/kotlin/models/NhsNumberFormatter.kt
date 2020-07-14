package models

class NhsNumberFormatter {
    companion object {
        private const val lengthOfNHSNumber = 10
        private const val firstNHSNumberIndex = 0
        private const val firstNHSNumberFormattingBreak = 3
        private const val secondNHSNumberFormattingBreak = 6
        private const val finalNHSNumberBreak = 10
        fun format(nhsNumber: String): String {
            val number = nhsNumber.trim().replace(" ", "")

            return when {
                number.isEmpty() -> ""
                number.length != lengthOfNHSNumber -> number
                else -> "${number.substring(firstNHSNumberIndex, firstNHSNumberFormattingBreak)} " +
                        "${number.substring(firstNHSNumberFormattingBreak, secondNHSNumberFormattingBreak)} " +
                        number.substring(secondNHSNumberFormattingBreak, finalNHSNumberBreak)
            }
        }
    }
}
