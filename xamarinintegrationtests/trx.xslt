<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:test="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">

    <xsl:output method="text" encoding="UTF-8" omit-xml-declaration="yes" indent="no"/>

    <xsl:template match="/">
        <xsl:text>[</xsl:text>
            <xsl:for-each select="//test:UnitTestResult[contains(test:Output/test:StdOut, 'TestReport:')]">
                <xsl:if test="position() &gt; 1">,</xsl:if>
                <xsl:value-of select="substring-after(test:Output/test:StdOut, 'TestReport:')" />
            </xsl:for-each>
        <xsl:text>]</xsl:text>
    </xsl:template>
</xsl:stylesheet>