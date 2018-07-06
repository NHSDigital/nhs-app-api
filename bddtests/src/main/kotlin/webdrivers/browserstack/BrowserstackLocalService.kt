package webdrivers.browserstack

import com.browserstack.local.Local

class BrowserstackLocalService {
    companion object {
        @JvmStatic fun main(arguments: Array<String>) {
            val bsLocalArgs = HashMap<String, String>()
            bsLocalArgs.put("key", arguments[0])

            start(bsLocalArgs)
        }

        private val bsLocal: Local = Local()

        fun start(args: HashMap<String, String>) {
            if (!bsLocal.isRunning) {
                bsLocal.start(args)
            } else {
                println("Browserstack local service already running.")
            }
        }

        fun stop() {
            bsLocal.stop()
        }

        fun isRunning(): Boolean {
            return bsLocal.isRunning
        }
    }
}
