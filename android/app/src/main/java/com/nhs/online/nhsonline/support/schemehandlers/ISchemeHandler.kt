package com.nhs.online.nhsonline.support.schemehandlers

interface ISchemeHandler
{
    val scheme: String

    fun handle(url: String): Boolean
}