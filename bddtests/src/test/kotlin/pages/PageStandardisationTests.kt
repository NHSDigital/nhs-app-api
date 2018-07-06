package pages

import com.google.common.reflect.ClassPath
import net.serenitybdd.core.annotations.findby.FindBy
import org.junit.Assert
import org.junit.Test
import org.junit.experimental.categories.Category
import org.junit.runner.RunWith
import org.junit.runners.JUnit4
import java.lang.reflect.Field

@RunWith(JUnit4::class)
@Category(PageObjectTests::class)
class PageStandardisationTests {

    @Test
    fun allPageClassesExtendHybridPageObject() {
        val classPath = ClassPath.from(this.javaClass.classLoader)

        val classes = classPath.getTopLevelClassesRecursive("pages")
                .filter { it.load().isInstance(HybridPageObject) }

        Assert.assertTrue("There were page objects that don't extend the HybridPageObject class.  Please change these.\n" +
                "Offending classes:\n" +
                "$classes",
                classes.isEmpty())
    }

    @Test
    fun allPageFieldsUseHybridPageElements() {
        val classPath = ClassPath.from(this.javaClass.classLoader)

        val fieldViolations = mutableMapOf<ClassPath.ClassInfo, MutableSet<Field>>()

        classPath.getTopLevelClassesRecursive("pages").forEach {
            fieldViolations[it] = mutableSetOf()

            for (field in it.load().declaredFields) {
                for (annotation in field.annotations) {
                    if (annotation.annotationClass == FindBy::class) {
                        fieldViolations[it]!!.add(field)
                    }
                }
            }
        }

        val output = fieldViolations
                .filter { it.value.isNotEmpty() }
                .map { "${it.key}: ${it.value.map { it.name }}" }
                .toList()

        Assert.assertTrue("There were fields that tried to use the Serenity @FindBy annotation." +
                "  Please convert these to use the new HybridPageElement class.\n" +
                "Offending Fields:\n${output.map { it.plus("\n") }}",
                output.isEmpty())
    }
}