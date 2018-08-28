package mocking.stubs

import mocking.models.Mapping

class InputResponse<TInput, TResponse>
{
    private val list : ArrayList<MockingPair<TInput, TResponse>> = arrayListOf()

    fun addResponse(forMatcher: TInput, getResponse: (TResponse) -> Mapping):InputResponse<TInput, TResponse>{
        list.add(MockingPair(forMatcher, getResponse))
        return this
    }

    fun listResponse(): ArrayList<MockingPair<TInput, TResponse>> {
        return list
    }

}

data class MockingPair<TInput, TResponse>(val forMatcher: TInput, val getResponse: (TResponse) -> Mapping)
