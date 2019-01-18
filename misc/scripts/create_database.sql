CREATE TABLE identity_roles (
    id uuid NOT NULL PRIMARY KEY,
    name character varying(256) UNIQUE NULL,
    normalized_name character varying(256) NULL,
    concurrency_stamp text NULL
);

CREATE TABLE identity_users (
    id uuid NOT NULL PRIMARY KEY,
    username character varying(256) NULL,
    normalized_username character varying(256) UNIQUE NULL,
    email character varying(256) NULL,
    normalized_email character varying(256) NULL,
    email_confirmed boolean NOT NULL,
    password_hash text NULL,
    security_stamp text NULL,
    concurrency_stamp text NULL,
    phone_number text NULL,
    phone_number_confirmed boolean NOT NULL,
    two_factor_enabled boolean NOT NULL,
    lockout_end timestamp with time zone NULL,
    lockout_enabled boolean NOT NULL,
    access_failed_count integer NOT NULL
);

CREATE TABLE identity_role_claims (
    id integer NOT NULL PRIMARY KEY,
    role_id uuid NOT NULL,
    claim_type text NULL,
    claim_value text NULL,
    CONSTRAINT fk_identity_role_claims_identity_roles_role_id FOREIGN KEY (role_id) REFERENCES identity_roles (id) ON DELETE CASCADE
);

CREATE TABLE identity_user_claims (
    id integer NOT NULL PRIMARY KEY,
    user_id uuid NOT NULL,
    claim_type text NULL,
    claim_value text NULL,
    CONSTRAINT fk_identity_user_claims_identity_users_user_id FOREIGN KEY (user_id) REFERENCES identity_users (id) ON DELETE CASCADE
);

CREATE TABLE identity_user_logins (
    login_provider character varying(128) NOT NULL,
    provider_key character varying(128) NOT NULL,
    provider_display_name text NULL,
    user_id uuid NOT NULL,
    CONSTRAINT pk_identity_user_logins PRIMARY KEY (login_provider, provider_key),
    CONSTRAINT fk_identity_user_logins_identity_users_user_id FOREIGN KEY (user_Id) REFERENCES identity_users (id) ON DELETE CASCADE
);

CREATE TABLE identity_user_roles (
    user_id uuid NOT NULL,
    role_id uuid NOT NULL,
    CONSTRAINT pk_identity_user_roles PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_identity_user_roles_identity_roles_role_id FOREIGN KEY (role_id) REFERENCES identity_roles (id) ON DELETE CASCADE,
    CONSTRAINT fk_identity_user_roles_identity_users_user_id FOREIGN KEY (user_id) REFERENCES identity_users (id) ON DELETE CASCADE
);

CREATE TABLE identity_user_tokens (
    user_id uuid NOT NULL,
    login_provider character varying(128) NOT NULL,
    name character varying(128) NOT NULL,
    value text NULL,
    CONSTRAINT pk_identity_tokens PRIMARY KEY (user_id, login_provider, name),
    CONSTRAINT fk_identity_tokens_identity_users_user_id FOREIGN KEY (user_id) REFERENCES identity_users (id) ON DELETE CASCADE
);

CREATE INDEX ix_identity_role_claims_role_id ON identity_role_claims (role_id);

CREATE INDEX ix_identity_user_claims_user_id ON identity_user_claims (user_id);

CREATE INDEX ix_identity_user_logins_user_id ON identity_user_logins (user_id);

CREATE INDEX ix_identity_user_roles_role_id ON identity_user_roles (role_id);

CREATE INDEX email_index ON identity_users (normalized_email);

GRANT ALL ON TABLE public.identity_role_claims TO sampledb;

GRANT ALL ON TABLE public.identity_roles TO sampledb;

GRANT ALL ON TABLE public.identity_user_claims TO sampledb;

GRANT ALL ON TABLE public.identity_user_logins TO sampledb;

GRANT ALL ON TABLE public.identity_user_roles TO sampledb;

GRANT ALL ON TABLE public.identity_user_tokens TO sampledb;

GRANT ALL ON TABLE public.identity_users TO sampledb;


