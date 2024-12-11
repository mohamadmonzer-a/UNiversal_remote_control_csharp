import mysql.connector
import sys

def get_instructors():
    # MySQL connection configuration
    mysql_config = {
        'host': 'localhost',
        'user': 'root',
        'password': 'admin',
        'database': 'seniorproject'  # Change this to your database name
    }

    try:
        connection = mysql.connector.connect(**mysql_config)
        cursor = connection.cursor()

        # Query to retrieve instructor names
        query = "SELECT image_username FROM Images"

        cursor.execute(query)
        instructors = [row[0] for row in cursor.fetchall()]

        return instructors

    except mysql.connector.Error as err:
        print(f"Error: {err}")
        return []

    finally:
        if 'connection' in locals() and connection.is_connected():
            cursor.close()
            connection.close()

def get_image_path(instructor_name):
    # MySQL connection configuration
    mysql_config = {
        'host': 'localhost',
        'user': 'root',
        'password': 'admin',
        'database': 'seniorproject'  # Change this to your database name
    }

    try:
        connection = mysql.connector.connect(**mysql_config)
        cursor = connection.cursor()

        # Query to retrieve image path based on instructor name
        query = "SELECT image_name FROM Images WHERE image_username = %s"
        cursor.execute(query, (instructor_name,))

        result = cursor.fetchone()
        if result:
            return result[0]  # Return the image path

        return None

    except mysql.connector.Error as err:
        print(f"Error: {err}")
        return None

    finally:
        if 'connection' in locals() and connection.is_connected():
            cursor.close()
            connection.close()

# Entry point for the script
if __name__ == "__main__":
    if len(sys.argv) == 1:
        # Fetch and print all instructor names
        instructors_list = get_instructors()
        for instructor in instructors_list:
            print(instructor)
    elif len(sys.argv) == 2:
        # Fetch image path based on the selected instructor name
        instructor_name = sys.argv[1]
        image_path = get_image_path(instructor_name)
        if image_path:
            print(image_path)
