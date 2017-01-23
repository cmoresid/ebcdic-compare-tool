CREATE TABLE CopybookEbcdicFileAssociation
(
	copybook_file_path VARCHAR(255),
	ebcdic_file_name VARCHAR(255),
	PRIMARY KEY (copybook_file_path, ebcdic_file_name)
);