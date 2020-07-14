package worker

data class JsonPatch(val op: JsonPatchOperation, val path:String, val value:Boolean)
