import mysql.connector
import json
import sys

def get_commands():
    try:
        # MySQL database configuration
        mysql_config = {
            'host': sys.argv[1],
            'user': 'root',
            'password': 'root',
            'database': 'saved_commands'
        }

        # Connect to MySQL
        connection = mysql.connector.connect(**mysql_config)
        cursor = connection.cursor()

        # Execute the SQL query to retrieve data
        cursor.execute("SELECT user_command_csharp, script FROM saved_commands_script")

        # Fetch all rows
        rows = cursor.fetchall()

        # Create a list to store the results
        results = []
        for row in rows:
            # Extract user_command_csharp and script from each row
            user_command_csharp, script = row
            # Append the data to the results list as a dictionary
            results.append({'user_command_csharp': user_command_csharp, 'script': script})

        # Close cursor and connection
        cursor.close()
        connection.close()

        # Return results as JSON string
        return json.dumps(results)

    except mysql.connector.Error as e:
        print(f"Error retrieving data: {e}")
        return None

# Example usage
if __name__ == "__main__":
    commands_json = get_commands()
    if commands_json:
        print(commands_json)
    else:
        print("No commands retrieved.")
