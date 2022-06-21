# LokiGraf.API
docker-compose.yaml поднимает связку Loki-Grafana-Promtail для тестирования


Filter by fields:
{app="LokiGraf.Console"}| logfmt | text = "Foobar"

{app="LokiGraf.Console"} |= "foobar"

https://grafana.com/docs/loki/latest/logql/log_queries/
