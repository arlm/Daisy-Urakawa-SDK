<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml"  />

  <xsl:template match="/">
    <wrapper>
      <ncx>
        <!-- Does the head, doctitle and docAuthor-->
        <xsl:apply-templates />
        <navMap>
          <xsl:apply-templates mode="NAVMAP" />
        </navMap>
      </ncx>
      <smil>
        <xsl:apply-templates mode="SMIL" />
      </smil>
    </wrapper>
  </xsl:template>

  <!-- Building the NAVMAP-->
  <xsl:template match="CoreNode[mProperties/ChannelsProperty/ChannelMapping/TextMedia]" mode="NAVMAP">
    <navTarget>
      <xsl:for-each select="mProperties/ChannelsProperty/ChannelMapping/TextMedia[1]">
        <navLabel>
          <text><xsl:value-of select="."/></text>
          <!-- Do something for Audio(?), even if current impl hasn't anything in direct sync -->
        </navLabel>
        <content>
          <xsl:attribute name="src">everything.smil#<xsl:value-of select ="id(ancestor::CoreNode[1])"/></xsl:attribute>
          
        </content>
        <xsl:apply-templates mode="NAVMAP" />
      </xsl:for-each>
    </navTarget>
  </xsl:template>
  
  <xsl:template match="*" mode="NAVMAP">
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on NAVMAP</xsl:message>
    <xsl:apply-templates mode="NAVMAP" />
  </xsl:template>


  
  
  <!-- Building the SMIL-->
  <xsl:template match="CoreNode" mode="SMIL">
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="id(.)"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="ChannelsProperty" mode="SMIL">
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="id(.)"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="SequenceMedia" mode="SMIL">
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="id(.)"/>
        <!-- not really needed, since nothing will be referring this seq directly -->
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <xsl:template match="AudioMedia" mode="SMIL">
    <audio>
      <xsl:copy-of select="@*"/>
    </audio>
  </xsl:template>

  <xsl:template match="ChannelMapping" mode="SMIL">
    <seq>
      <xsl:attribute name="id">
        <xsl:value-of select="id(.)"/>
      </xsl:attribute>
      <xsl:apply-templates mode="SMIL" />
    </seq>
  </xsl:template>

  <!-- Currently Obi does not produce fulltext, so there is little reason to include references to such a file
  
  <xsl:template match="TextMedia" mode="SMIL">
    <text>
      <xsl:attribute name="src">
        <xsl:value-of select="GetTheNameOfTheFulltextDoc"/>#<xsl:value-of select="id(ancestor::CoreNode[1])"/>
      </xsl:attribute>
    </text>
  </xsl:template>
  -->



  <xsl:template match="*" mode="SMIL">
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on SMIL</xsl:message>
    <xsl:apply-templates mode="SMIL" />
  </xsl:template>
  
  <xsl:template match="*">
    <xsl:message terminate="no" >Processing <xsl:value-of select="name()"/> on *</xsl:message>
    <xsl:apply-templates />
  </xsl:template>


</xsl:stylesheet>