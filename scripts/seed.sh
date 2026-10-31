#!/usr/bin/env bash
# Seeds test menus and orders through the running API.
# Usage: ./scripts/seed.sh [base-url]   (default: http://localhost:5262)
set -euo pipefail

BASE_URL="${1:-http://localhost:5262}"
JSON='Content-Type: application/json'

post() { curl -sf -X POST "$BASE_URL$1" -H "$JSON" -d "$2"; }
id_of() { python3 -c 'import sys,json; print(json.load(sys.stdin)["id"])'; }

create_menu() { post /menus "{\"name\":\"$1\",\"description\":\"$2\",\"price\":$3}" | id_of; }

# create_order <menuId> <quantity> <date> <finalStatus>
create_order() {
  local order_id
  order_id=$(post /orders "{\"menuId\":\"$1\",\"status\":\"Pending\",\"quantity\":$2,\"date\":\"$3\"}" | id_of)
  if [ "$4" != "Pending" ]; then
    curl -sf -X PATCH "$BASE_URL/orders/$order_id" -H "$JSON" -d "{\"status\":\"$4\"}" > /dev/null
  fi
}

echo "Creating menus..."
PIZZA=$(create_menu "Margherita Pizza" "Tomato, mozzarella and basil" 9.50)
BURGER=$(create_menu "Cheeseburger" "Beef patty with cheddar and fries" 12.00)
SALAD=$(create_menu "Caesar Salad" "Romaine, croutons and parmesan" 8.25)
PASTA=$(create_menu "Spaghetti Carbonara" "Egg, pecorino and guanciale" 11.75)

echo "Creating orders..."
# Pending
create_order "$PIZZA"  2 2026-10-29 Pending
create_order "$SALAD"  1 2026-10-30 Pending
create_order "$BURGER" 3 2026-10-31 Pending
# InProgress
create_order "$PASTA"  2 2026-10-30 InProgress
create_order "$BURGER" 1 2026-10-31 InProgress
# Completed
create_order "$PIZZA"  4 2026-10-27 Completed
create_order "$PIZZA"  1 2026-10-28 Completed
create_order "$BURGER" 2 2026-10-28 Completed
create_order "$PASTA"  3 2026-10-29 Completed
create_order "$SALAD"  2 2026-10-29 Completed
create_order "$BURGER" 1 2026-10-30 Completed
create_order "$PASTA"  1 2026-10-31 Completed
# Cancelled
create_order "$SALAD"  5 2026-10-28 Cancelled
create_order "$PIZZA"  2 2026-10-30 Cancelled

echo "Done: 4 menus, 14 orders."
echo "Try: $BASE_URL/reports/summary?startDate=2026-10-27&endDate=2026-10-31"
