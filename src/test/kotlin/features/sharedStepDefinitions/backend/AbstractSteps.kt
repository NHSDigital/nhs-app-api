package features.sharedStepDefinitions.backend

import mocking.defaults.MockDefaults
import mocking.MockingClient
import net.serenitybdd.core.Serenity
import worker.WorkerClient

abstract class AbstractSteps {

    private val context: MutableMap<Any, Any> get() = Serenity.getCurrentSession()

    private inline fun <reified T> getFromContext(throwIfNull: Boolean = true): T {
        val type = T::class
        val value = context.get(type.qualifiedName!!)
        if (value == null && throwIfNull) throw IllegalArgumentException("No value in context for ${type.qualifiedName}")
        return value as T
    }

    private fun <T : Any> setInContext(value: T) {
        context[value.javaClass.kotlin.qualifiedName!!] = value
    }

    protected var mockingClient: MockingClient
        get() = getFromContext()
        set(value) = setInContext(value)

    protected var workerClient: WorkerClient
        get() = getFromContext()
        set(value) = setInContext(value)

    companion object {
        val connectionToken: String = MockDefaults.DEFAULT_CONNECTION_TOKEN
        val odsCode: String = MockDefaults.DEFAULT_ODS_CODE
    }
}
