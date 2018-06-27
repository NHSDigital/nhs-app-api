package com.nhs.online.nhsonline

import android.os.Parcel
import android.view.View
import com.nhs.online.nhsonline.navigation.MenuBar
import com.nhs.online.nhsonline.support.Optional
import org.junit.Assert.*
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class SaveStateTest {

    @Test
    fun savedStatePositionRestores() {
        actAndAssertCanSaveAndRestorePosition(24)
    }

    @Test
    fun maxSavedStatePositionRestores() {
        actAndAssertCanSaveAndRestorePosition(Int.MAX_VALUE)
    }

    @Test
    fun minSavedStatePositionRestores() {
        actAndAssertCanSaveAndRestorePosition(Int.MIN_VALUE)
    }

    @Test
    fun emptySavedStatePositionRestores() {
        val restoredState = saveStateAndRestore(Optional.empty())
        restoredState.selectedPosition.ifPresent {
            fail("Expected restoredState.selectedPosition to be empty")
        }
    }

    private fun actAndAssertCanSaveAndRestorePosition(position: Int) {
        val restoredState = saveStateAndRestore(Optional.of(position))

        restoredState.selectedPosition.ifPresent { restoredPosition ->
            assertEquals(position, restoredPosition)
        }
        restoredState.selectedPosition.ifEmpty {
            fail("Expected restoredState.selectedPosition to be non-empty")
        }
    }

    private fun saveStateAndRestore(position: Optional<Int>): MenuBar.SavedState {
        val parcel = Parcel.obtain()
        val baseSavedParcelable = View.BaseSavedState(parcel)
        val originalState = MenuBar.SavedState(
                baseSavedParcelable,
                position
        )
        originalState.writeToParcel(parcel, 0)
        parcel.setDataPosition(0)

        return MenuBar.SavedState(parcel)
    }
}