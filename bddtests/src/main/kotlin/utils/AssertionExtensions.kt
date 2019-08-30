package utils

import org.junit.Assert.assertTrue

const val DEFAULT_WAIT_TIME = 500L
const val DEFAULT_RETRIES = 3

fun assertTrueWithRetry(condition: Boolean,
                        message:String,
                        numberOfRetries: Int = DEFAULT_RETRIES,
                        waitTime: Long = DEFAULT_WAIT_TIME ) {
    var retryCountdown = numberOfRetries
    while(retryCountdown>0) {
        try {
            assertTrue(message, condition)
            retryCountdown=0
        }
        catch(e: AssertionError) {
            println("Could not assert true. RETRYING")
            Thread.sleep(waitTime)
            retryCountdown--
            if(retryCountdown==0)
                throw e
        }
    }
}
