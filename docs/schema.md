# Database Schema

## businesses
- id PK
- name

## customers
- id PK
- full_name
- email unique

## customer_packages
- id PK
- customer_id FK customers.id
- business_id FK businesses.id
- total_credits
- remaining_credits
- expiry_date

## timetable_schedules
- id PK
- business_id FK businesses.id
- class_name
- instructor_name
- start_time
- end_time
- available_slots

## bookings
- id PK
- customer_id FK customers.id
- customer_package_id FK customer_packages.id
- timetable_schedule_id FK timetable_schedules.id
- status
- booked_at
- cancelled_at nullable
- credit_refunded

## waitlist_entries
- id PK
- customer_id FK customers.id
- customer_package_id FK customer_packages.id
- timetable_schedule_id FK timetable_schedules.id
- status
- joined_at
- promoted_at nullable
