# NHS App - Xamarin

## General Principles

There are a few general principles to adhere to when working in the Xamarin app and associated integration tests

* Keep it simple
  * There should be as little magic as possible
  * It should be easy for someone new to follow the code and understand what's happening
* Keep it explicit
  * Whilst abstractions and code re-use are laudable, these things are nearly always a tradeoff with simplicity and understandability
* Keep to the existing patterns in place
  * A consistent code base is much easier to understand than one with many different styles or patterns
  * New things can be introduced, of course, but these should be justified and discussed amongst the team

The theme here is making it easy for future developers to pick up the code and understand it to make changes.

## Xamarin patterns

It is essential to understand, or at least be aware of, the [fundamentals of Xamarin](https://docs.microsoft.com/en-us/xamarin/get-started/what-is-xamarin) before getting started.

Using these fundamentals there are many ways that any given feature can be implemented in Xamarin and so the following serves to document that ways that have been chosen and proven on the NHS App.

* [Threading](xamarin/threading-pattern.md)
* [Model View Presenter](xamarin/model-view-presenter.md)
* [Data Binding](xamarin/data-binding.md)
* [Integration Tests](xamarin/integration-tests.md)

## Setup

* [Environment Setup](xamarin/environment-setup.md)
