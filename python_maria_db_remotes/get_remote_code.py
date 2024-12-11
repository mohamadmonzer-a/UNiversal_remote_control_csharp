import mysql.connector
import re
import sys

def search_buttons(manufacturer, button_key):
    # Define the search terms based on the button key
    search_terms = button_key.split(',')  # Split button key into individual terms
    manufacturer_pattern = re.compile(manufacturer, re.IGNORECASE)

    # Connect to the MariaDB database
    conn = mysql.connector.connect(
        host='192.168.31.211',  # Change this to your MySQL host address
        user='root',
        password='root',
        database='remote_ir'
    )
    cursor = conn.cursor()

    # Retrieve all table names
    cursor.execute("SHOW TABLES")
    all_tables = cursor.fetchall()

    # Set to store unique buttons found
    unique_buttons = set()

    # Set to store remotes containing the buttons
    remotes_with_buttons = set()

    # Iterate over the tables and search for buttons based on each term
    for table_info in all_tables:
        table_name = table_info[0]  # Extracting table name from the tuple
        if re.match(manufacturer_pattern, table_name):
            # Construct the WHERE clause for the SQL query dynamically
            where_clause = " OR ".join([f"LOWER(functionname) LIKE '%{term}%'" for term in search_terms])
            query = f"SELECT * FROM `{table_name}` WHERE {where_clause}"
            cursor.execute(query)
            found_buttons = cursor.fetchall()

            # Add found buttons to the set
            for button in found_buttons:
                remotes_with_buttons.add(table_name)  # Add remote name
                unique_buttons.add((table_name, button))  # Add remote name and button info

    # Print the unique buttons
    for remote, button in unique_buttons:
        print(f"{remote}: {button}")

    # Print the remotes containing the buttons
    print("\nRemotes containing the buttons:")
    for remote in remotes_with_buttons:
        print(remote)

    # Close the connection
    conn.close()

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python script_name.py manufacturer_name button_key")
        sys.exit(1)
    
    manufacturer_name = sys.argv[1]
    button_key = sys.argv[2]
    search_buttons(manufacturer_name, button_key)
