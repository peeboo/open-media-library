<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes"/>
	<xsl:template match="/">
		<xsl:element name="OMLTitle">
			<xsl:for-each select="//Title">
				<xsl:element name="Movie">
					<xsl:attribute name="IsUpdated"></xsl:attribute>
					<xsl:element name="Title">
						<xsl:value-of select="OriginalTitle"/>
					</xsl:element>
					<xsl:element name="SortAs">
						<xsl:value-of select="SortTitle"/>
					</xsl:element>
					<xsl:element name="Disks">
						<xsl:for-each select="Discs/Disc">
							<xsl:element name="Disk">
								<xsl:attribute name="id"></xsl:attribute>
								<xsl:element name="Location">
									<xsl:value-of select="LocationSideA"/>
								</xsl:element>
								<xsl:element name="Format">
								</xsl:element>
							</xsl:element>
						</xsl:for-each>
					</xsl:element>
					<xsl:element name="Location">
					</xsl:element>
					<xsl:element name="Cover">
						<xsl:element name="Front">
							<xsl:value-of select="Covers/Front"/>
						</xsl:element>
						<xsl:element name="Back">
							<xsl:value-of select="Covers/Back"/>
						</xsl:element>
					</xsl:element>
					<xsl:element name="Aspect">
					</xsl:element>
					<xsl:element name="Runtime">
						<xsl:value-of select="RunningTime"/>
					</xsl:element>
					<xsl:element name="MPAA">
						<xsl:attribute name="Rating">
							<xsl:value-of select="ParentalRating/Value"/>
						</xsl:attribute>
						<xsl:value-of select="ParentalRating/Description"/>
					</xsl:element>
					<xsl:element name="Synopsis">
						<xsl:value-of select="Description"/>
					</xsl:element>
					<xsl:element name="Genres">
						<xsl:for-each select="Genres">
							<xsl:element name="Genre">
								<xsl:value-of select="Genre"/>
							</xsl:element>
						</xsl:for-each>
					</xsl:element>

					<xsl:element name="Persons">
						<xsl:for-each select="Persons/Person">
							<xsl:element name="Person">
								<xsl:attribute name="IsActor"></xsl:attribute>
								<xsl:attribute name="IsDirector"></xsl:attribute>
								<xsl:attribute name="IsProducer"></xsl:attribute>
								<xsl:attribute name="IsWriter"></xsl:attribute>
								<xsl:attribute name="Gender"></xsl:attribute>
								<xsl:attribute name="Birthdate"></xsl:attribute>
								<xsl:attribute name="Uncredited"></xsl:attribute>
								<xsl:element name="Name">
									<xsl:value-of select="Name"/>
								</xsl:element>
								<xsl:element name="Role">
									<xsl:value-of select="Role"/>
								</xsl:element>
							</xsl:element>
						</xsl:for-each>
					</xsl:element>
				</xsl:element>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>