--liquibase formatted sql

--changeset system:030-partition-alert-state
-- Состояние alert'ов PartitionHealthCheck. Хранится в БД, а не в памяти:
-- перезапуск сервиса не должен приводить к повторной рассылке уже
-- отправленного alert'а, а несколько экземпляров должны видеть общую картину.
CREATE TABLE IF NOT EXISTS partition_alert_state (
    alert_key        varchar(200) PRIMARY KEY,
    status           varchar(20)  NOT NULL,
    details          text,
    first_seen_at    timestamptz  NOT NULL,
    last_notified_at timestamptz,
    notify_count     integer      NOT NULL DEFAULT 0,
    updated_at       timestamptz  NOT NULL
);
--rollback DROP TABLE IF EXISTS partition_alert_state;

--changeset system:031-v-partitions
-- Витрина для дежурного: какие партиции существуют и сколько занимают.
CREATE OR REPLACE VIEW v_partitions AS
SELECT p.relname                          AS parent_table,
       c.relname                          AS partition_name,
       pg_get_expr(c.relpartbound, c.oid) AS bounds,
       pg_total_relation_size(c.oid)      AS size_bytes,
       c.reltuples::bigint                AS approx_rows
FROM pg_class c
JOIN pg_inherits i  ON i.inhrelid = c.oid
JOIN pg_class p     ON p.oid = i.inhparent
JOIN pg_namespace n ON n.oid = p.relnamespace
WHERE c.relkind = 'r'
ORDER BY p.relname, c.relname;
--rollback DROP VIEW IF EXISTS v_partitions;
