CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(100) NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'user',
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    row_version BIGINT NOT NULL DEFAULT 1
);

CREATE TABLE schedules (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP NOT NULL,
    row_version BIGINT NOT NULL DEFAULT 1
);

CREATE TABLE attractions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    schedule_id UUID NOT NULL REFERENCES schedules(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    image_url TEXT,
    capacity INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    row_version BIGINT NOT NULL DEFAULT 1
);

CREATE TABLE registrations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    attraction_id UUID NOT NULL REFERENCES attractions(id) ON DELETE CASCADE,
    registered_at TIMESTAMP NOT NULL DEFAULT NOW(),
    row_version BIGINT NOT NULL DEFAULT 1
);

ALTER TABLE registrations
ADD CONSTRAINT uq_user_attraction UNIQUE (user_id, attraction_id);

CREATE OR REPLACE FUNCTION update_row_version()
RETURNS TRIGGER AS $$
BEGIN
    NEW.row_version := OLD.row_version + 1;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Attach triggers to each table
CREATE TRIGGER trg_users_rowversion
BEFORE UPDATE ON users
FOR EACH ROW EXECUTE FUNCTION update_row_version();

CREATE TRIGGER trg_schedules_rowversion
BEFORE UPDATE ON schedules
FOR EACH ROW EXECUTE FUNCTION update_row_version();

CREATE TRIGGER trg_attractions_rowversion
BEFORE UPDATE ON attractions
FOR EACH ROW EXECUTE FUNCTION update_row_version();

CREATE TRIGGER trg_registrations_rowversion
BEFORE UPDATE ON registrations
FOR EACH ROW EXECUTE FUNCTION update_row_version();