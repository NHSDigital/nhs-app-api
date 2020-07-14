package worker.models.clientLogger

data class ClientLoggerRequest(val timeStamp: String,
                               val level: String,
                               val message: String)
