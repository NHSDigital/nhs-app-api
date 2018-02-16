package com.nhs.online.nhsonline.support

abstract class Optional<T> {

    companion object {
        fun <T> empty(): Optional<T> {
            return Empty()
        }

        fun <T> of(value: T): Optional<T> {
            return Some(value)
        }
    }

    abstract fun ifPresent(doIfPresent: (T) -> Unit)
    abstract fun ifEmpty(doIfEmpty: () -> Unit)
    abstract fun orElse(defaultValue: T): T

    class Some<T>(private val value: T) : Optional<T>() {
        override fun orElse(defaultValue: T): T {
            return value
        }

        override fun ifEmpty(doIfEmpty: () -> Unit) {
            // Intentionally empty as doIfEmpty should not be invoked
        }

        override fun ifPresent(doIfPresent: (T) -> Unit) {
            doIfPresent(value)
        }
    }

    class Empty<T> : Optional<T>() {
        override fun orElse(defaultValue: T): T {
            return defaultValue
        }

        override fun ifEmpty(doIfEmpty: () -> Unit) {
            doIfEmpty()
        }

        override fun ifPresent(doIfPresent: (T) -> Unit) {
            // Intentionally empty as doIfPresent should not be invoked
        }
    }
}

