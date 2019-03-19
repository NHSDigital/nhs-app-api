<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:variable name="apostrophe">'</xsl:variable>
    <xsl:output method="text" omit-xml-declaration="yes" encoding="UTF8" indent="no"/>

    <xsl:template match="/coverage">

        <xsl:text>
            ##teamcity[blockOpened name='Code Coverage Summary']
        </xsl:text>

        <xsl:comment>
            REFERENCES

            Details on how the surmmary is rendered on Overview screen
            http://gotwarlost.github.io/istanbul/public/apidocs/files/lib_report_teamcity.js.html

        </xsl:comment>

        <xsl:comment>
            LINES
        </xsl:comment>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsLCovered', $apostrophe,
        ' value=', $apostrophe, @lines-covered, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsLTotal', $apostrophe,
        ' value=', $apostrophe, @lines-valid, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageL', $apostrophe,
        ' value=', $apostrophe, @line-rate * 100, $apostrophe,
        ']','&#10;')"/>

        <xsl:comment>
            BRANCHES
        </xsl:comment>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsRCovered', $apostrophe,
        ' value=', $apostrophe, @branches-covered, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsRTotal', $apostrophe,
        ' value=', $apostrophe, @branches-valid, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageR', $apostrophe,
        ' value=', $apostrophe, @branch-rate * 100, $apostrophe,
        ']','&#10;')"/>


        <xsl:text>
            ##teamcity[blockClosed name='Code Coverage Summary']
        </xsl:text>

    </xsl:template>
</xsl:stylesheet>
