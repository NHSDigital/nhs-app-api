package utils

import cucumber.api.DataTable

fun DataTable.toMap(): Map<String, String> {
    return this.raw().map { pair -> pair!![0]!! to pair[1]!! }.toMap()
}

fun DataTable.toSingleElementList(): List<String> {
    return this.raw().map { row -> row!![0]!! }
}