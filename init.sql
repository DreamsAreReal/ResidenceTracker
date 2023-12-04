DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = '__EFMigrationsHistory') THEN
CREATE TABLE "__EFMigrationsHistory" (
                                         "MigrationId" character varying(150) NOT NULL,
                                         "ProductVersion" character varying(32) NOT NULL,
                                         CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Houses') THEN
CREATE TABLE "Houses" (
                          "Id" uuid NOT NULL,
                          "Number" integer NOT NULL,
                          "CreatedAt" timestamp with time zone NOT NULL,
                          "IsReadonly" boolean NOT NULL,
                          "ModifiedAt" timestamp with time zone NOT NULL,
                          CONSTRAINT "PK_Houses" PRIMARY KEY ("Id")
);
END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Flats') THEN
CREATE TABLE "Flats" (
                         "Id" uuid NOT NULL,
                         "Number" integer NOT NULL,
                         "HouseId" uuid,
                         "CreatedAt" timestamp with time zone NOT NULL,
                         "IsReadonly" boolean NOT NULL,
                         "ModifiedAt" timestamp with time zone NOT NULL,
                         CONSTRAINT "PK_Flats" PRIMARY KEY ("Id"),
                         CONSTRAINT "FK_Flats_Houses_HouseId" FOREIGN KEY ("HouseId") REFERENCES "Houses" ("Id") ON DELETE RESTRICT
);
END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Bills') THEN
CREATE TABLE "Bills" (
                         "Id" uuid NOT NULL,
                         "AmountInRubles" numeric NOT NULL,
                         "FlatId" uuid NOT NULL,
                         "PaidIn" timestamp with time zone,
                         "CreatedAt" timestamp with time zone NOT NULL,
                         "IsReadonly" boolean NOT NULL,
                         "ModifiedAt" timestamp with time zone NOT NULL,
                         CONSTRAINT "PK_Bills" PRIMARY KEY ("Id"),
                         CONSTRAINT "FK_Bills_Flats_FlatId" FOREIGN KEY ("FlatId") REFERENCES "Flats" ("Id") ON DELETE RESTRICT
);
END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Members') THEN
CREATE TABLE "Members" (
                           "Id" uuid NOT NULL,
                           "Name" text NOT NULL,
                           "FlatId" uuid,
                           "CreatedAt" timestamp with time zone NOT NULL,
                           "IsReadonly" boolean NOT NULL,
                           "ModifiedAt" timestamp with time zone NOT NULL,
                           CONSTRAINT "PK_Members" PRIMARY KEY ("Id"),
                           CONSTRAINT "FK_Members_Flats_FlatId" FOREIGN KEY ("FlatId") REFERENCES "Flats" ("Id") ON DELETE RESTRICT
);
END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'ResidencyEventLogs') THEN
CREATE TABLE "ResidencyEventLogs" (
                                      "Id" uuid NOT NULL,
                                      "EventType" integer NOT NULL,
                                      "FlatId" uuid NOT NULL,
                                      "MemberId" uuid NOT NULL,
                                      "CreatedAt" timestamp with time zone NOT NULL,
                                      "IsReadonly" boolean NOT NULL,
                                      "ModifiedAt" timestamp with time zone NOT NULL,
                                      CONSTRAINT "PK_ResidencyEventLogs" PRIMARY KEY ("Id"),
                                      CONSTRAINT "FK_ResidencyEventLogs_Flats_FlatId" FOREIGN KEY ("FlatId") REFERENCES "Flats" ("Id") ON DELETE RESTRICT,
                                      CONSTRAINT "FK_ResidencyEventLogs_Members_MemberId" FOREIGN KEY ("MemberId") REFERENCES "Members" ("Id") ON DELETE RESTRICT
);
END IF;

END $$;


START TRANSACTION;


INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20231204013222_Initial', '8.0.0');

COMMIT;
