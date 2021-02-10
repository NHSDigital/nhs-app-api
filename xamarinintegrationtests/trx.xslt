<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:test="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">

    <xsl:output method="text" encoding="UTF-8" omit-xml-declaration="yes" indent="no"/>

    <xsl:template match="/">
        <xsl:text># Test Summary

## Retried Tests
</xsl:text>
        <xsl:apply-templates select="//test:UnitTestResult" mode="Retried" />
    </xsl:template>

    <!-- Retried tests are reported as not executed but have output from when they were executed -->
    <xsl:template match="test:UnitTestResult[@outcome='NotExecuted' and test:Output/test:StdOut]" mode="Retried">
        <xsl:variable name="testId" select="@testId" />
        <xsl:variable name="testName" select="//test:UnitTest[@id=$testId]/@name" />

        <xsl:text>
* </xsl:text>
        <xsl:value-of select="$testName" />
        <xsl:text>

    ```text
    </xsl:text>
        <xsl:value-of select="test:Output/test:ErrorInfo/test:Message" />
        <xsl:text>
    ```
</xsl:text>
    </xsl:template>

    <xsl:template match="test:UnitTestResult" mode="Retried" />

</xsl:stylesheet>