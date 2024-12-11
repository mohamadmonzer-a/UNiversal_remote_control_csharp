import mysql.connector
from flask import Flask, jsonify

app = Flask(__name__)

@app.route('/get_commands')
def get_commands():
    try:
        ip_address = sys.argv[1]  # Get the IP address from command-line arguments
        mysql_config = {
            'host': ip_address,  # Use the provided MySQL host address
            'user': 'root',
            'password': 'root',
            'database': 'saved_commands'  # Database name
        }


        cursor = connection.cursor()

        # Fetch commands from the database
        cursor.execute("SELECT user_command_csharp, broadlink_command FROM saved_commands_script")

        commands = []
        for row in cursor.fetchall():
            command = {
                'user_command_csharp': row[0],
                'broadlink_command': row[1]
            }
            commands.append(command)

        return jsonify(commands)

    except Exception as e:
        return jsonify({'error': str(e)})

    finally:
        cursor.close()
        connection.close()

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
