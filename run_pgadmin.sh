docker run -p 8053:80 \
    -e 'PGADMIN_DEFAULT_EMAIL=postgres@postgres.com' \
    -e 'PGADMIN_DEFAULT_PASSWORD=pa55word!' \
    -d dpage/pgadmin4
