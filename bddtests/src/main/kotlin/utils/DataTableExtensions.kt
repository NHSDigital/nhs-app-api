package utils

import io.cucumber.datatable.DataTable

fun DataTable.toMap(): Map<String, String> {
    return this.cells().map { pair -> pair!![0]!! to pair[1]!! }.toMap()
}

fun DataTable.toSingleElementList(): List<String> {
    return this.cells().map { row -> row!![0]!! }
}
