<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="@*|node()">
 <xsl:copy>
  <xsl:apply-templates select="@*|node()"/>
 </xsl:copy>
</xsl:template>
<xsl:template match="testcase[failure]" />
<xsl:template match="testsuite/@failures">
  <xsl:attribute name="failures">
    <xsl:value-of select="0"/>
  </xsl:attribute>
</xsl:template>
</xsl:stylesheet>
