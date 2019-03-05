<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:variable name="apostrophe">'</xsl:variable>
    <xsl:output method="text" omit-xml-declaration="yes" encoding="UTF8" indent="no"/>

    <xsl:template match="/CoverageSession/Summary">

        <xsl:text>
            ##teamcity[blockOpened name='Code Coverage Summary']
        </xsl:text>

        <xsl:comment>
            REFERENCES

            Details on how the surmmary is rendered on Overview screen
            http://gotwarlost.github.io/istanbul/public/apidocs/files/lib_report_teamcity.js.html

            Opencover and Team City
            https://geoffhudik.com/tech/2017/11/17/test-coverage-with-opencover-xunit-cake-and-teamcity/
        </xsl:comment>


        <xsl:comment>
            CLASSES
        </xsl:comment>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsCCovered', $apostrophe,
        ' value=', $apostrophe, @visitedClasses, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsCTotal', $apostrophe,
        ' value=', $apostrophe, @numClasses, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageC', $apostrophe,
        ' value=', $apostrophe, (@visitedClasses div @numClasses) * 100, $apostrophe,
        ']','&#10;')"/>


        <xsl:comment>
            METHODS
        </xsl:comment>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsMCovered', $apostrophe,
        ' value=', $apostrophe, @visitedMethods, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsCTotal', $apostrophe,
        ' value=', $apostrophe, @numMethods, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageM', $apostrophe,
        ' value=', $apostrophe, (@visitedMethods div @numMethods) * 100, $apostrophe,
        ']','&#10;')"/>


        <xsl:comment>
            SEQUENCE POINTS / STATEMENTS
        </xsl:comment>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsSCovered', $apostrophe,
        ' value=', $apostrophe, @visitedSequencePoints, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsSTotal', $apostrophe,
        ' value=', $apostrophe, @numSequencePoints, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageS', $apostrophe,
        ' value=', $apostrophe, (@visitedSequencePoints div @numSequencePoints) * 100, $apostrophe,
        ']','&#10;')"/>


        <xsl:comment>
            BRANCHES
        </xsl:comment>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsBCovered', $apostrophe,
        ' value=', $apostrophe, @visitedBranchPoints, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageAbsBTotal', $apostrophe,
        ' value=', $apostrophe, @numBranchPoints, $apostrophe,
        ']','&#10;')"/>

        <xsl:value-of select="concat(
        '##teamcity[buildStatisticValue',
        ' key=', $apostrophe, 'CodeCoverageB', $apostrophe,
        ' value=', $apostrophe, (@visitedBranchPoints div @numBranchPoints) * 100, $apostrophe,
        ']','&#10;')"/>


        <xsl:text>
            ##teamcity[blockClosed name='Code Coverage Summary']
        </xsl:text>

    </xsl:template>
</xsl:stylesheet>
