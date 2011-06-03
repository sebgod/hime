﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="ItemSet">
    <tr>
      <td>
          <xsl:attribute name="onclick">
            <xsl:text>display('Set_</xsl:text><xsl:value-of select="@SetID"/><xsl:text>.html')</xsl:text>
          </xsl:attribute>
          Set <xsl:value-of select="@SetID"/>
      </td>
    </tr>
  </xsl:template>
  
  <xsl:template match="LRGraph">
    <html xmlns="http://www.w3.org/1999/xhtml" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemalocation="http://www.w3.org/MarkUp/SCHEMA/xhtml11.xsd" xml:lang="en">
      <head>
        <title>Menu</title>
        <link rel="stylesheet" type="text/css" href="hime_data/Hime.css" />
        <script src="hime_data/Hime.js" type="text/javascript">aaa</script>
      </head>
      <body>
        <table style="float:left; width:150pt">
          <tr>
            <td onclick="display('grammar.html')">
              Grammar
            </td>
          </tr>
          <xsl:apply-templates/>
        </table>
        <div id="myContent">
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
