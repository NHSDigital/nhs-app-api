package webdrivers.options

import net.serenitybdd.core.Serenity
import kotlin.reflect.KClass

class OptionManager {

    private val optionsEnabled = HashSet<String>()
    private val options = HashMap<WebDriverOptionGroup, IWebDriverOption>()

    /**
     * @return true if this option should be configured at web driver initialisation.
     */
    fun <T : IWebDriverOption> isEnabled(optionType: KClass<T>): Boolean {
        return optionsEnabled.contains(optionType.java.name)
    }

    fun registerOption(option: IWebDriverOption) {
        options[option.group] = option
        optionsEnabled.add(option::class.java.name)
    }

    fun getOptions(): Collection<IWebDriverOption> {
        return options.values.toList()
    }

    companion object {
        fun instance(): OptionManager {

            val optionManager: OptionManager = Serenity.getCurrentSession()
                    .getOrDefault(OptionManager::class.java.name, OptionManager()) as OptionManager

            Serenity.setSessionVariable(OptionManager::class.java.name).to(optionManager)

            return optionManager
        }
    }
}
