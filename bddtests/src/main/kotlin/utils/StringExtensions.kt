package utils

fun String.contains(stringsToCheck: Array<String>): Boolean {
    stringsToCheck.forEach {
        if(this.contains(it))
            return true
    }
    return false
}