import os
import mysql.connector
import sys

# Check if the correct number of arguments is provided
if len(sys.argv) != 2:
    print("Usage: python script_name.py mysql_host")
    sys.exit(1)

# MySQL connection configuration
mysql_config = {
    'host': sys.argv[1],  # Use the provided MySQL host address
    'user': 'root',
    'password': 'root',
    'database': 'seniorproject'  # Change this to your database name
}

# Establishing a connection to MySQL
connection = mysql.connector.connect(**mysql_config)
cursor = connection.cursor()

# Create table if it doesn't exist
create_table_query = """
    CREATE TABLE IF NOT EXISTS Images (
        id INT AUTO_INCREMENT PRIMARY KEY,
        image_username VARCHAR(255),
        image_name VARCHAR(255),
        image_data MEDIUMBLOB
    )
"""

try:
    cursor.execute(create_table_query)
    connection.commit()
    print("Table created or already exists")
except mysql.connector.Error as err:
    print(f"Error: {err}")

# Directory containing the images
directory = r'C:\Users\Silver\Desktop\c# app\Mu.Project2\Mu.Project2\python_to_get_names_and_set_to_data_phpmyadmin\images'

# Iterate through the directory and process image files
for filename in os.listdir(directory):
    if filename.endswith('.jpg') or filename.endswith('.png') or filename.endswith('.jpeg'):
        # Insert image into the database
        try:
            with open(os.path.join(directory, filename), 'rb') as file:
                image_data = file.read()
                image_username = os.path.splitext(filename)[0]  # Extracting username from filename

                # Insert image into the table
                insert_query = "INSERT INTO Images (image_username, image_name, image_data) VALUES (%s, %s, %s)"
                cursor.execute(insert_query, (image_username, filename, image_data))
                connection.commit()
                print(f"Inserted {filename} into the database")
        except mysql.connector.Error as err:
            print(f"Error inserting {filename}: {err}")

# Close the cursor and connection
cursor.close()
connection.close()
