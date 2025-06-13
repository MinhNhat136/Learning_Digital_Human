
import os
import asyncio

from langchain_mcp_adapters.client import MultiServerMCPClient
from langchain_openai import ChatOpenAI
from langgraph.prebuilt import create_react_agent
from langgraph.checkpoint.memory import MemorySaver
from langchain.schema.messages import HumanMessage, AIMessage, ToolMessage
import nbimporter
import argparse
import socket


# # For pretty print all messages
# from rich import print
# from rich.panel import Panel
# from rich.pretty import Pretty

# Util functions for displaying responses
def msg_header(text, delimiter='=', width=50):
    bracket_text = f"[{text}]"

    padding = width - len(bracket_text)
    left_pad = padding // 2
    right_pad = padding - left_pad

    return delimiter * left_pad + bracket_text + delimiter * right_pad + '\n'

def pretty_print(messages):
    return messages[-1].content


# Functions to make a chatbot with MCP server, MCP client, chatbot app
async def connect_to_servers():
    """
    Connect to the servers and return the tools.
    """
    client = MultiServerMCPClient(
        {
            # # Connect to test MCP server
            # "math": {
            #     "command": "python",
            #     "args": [os.path.abspath("math_server.py")],
            #     "transport": "stdio",
            # },
            "conversation": {
                "command": "python",
                "args": [os.path.abspath("conversation_server.py")],
                "transport": "stdio",
            },
            "calculation": {
                "command": "python",
                "args": [os.path.abspath("calculation_server.py")],
                "transport": "stdio",
            },
        }
    )
    tools = await client.get_tools()
    return tools


def setup_agent_w_memory(tools):
    """
    Create ReAct agent with memory
    """
    import os
    os.environ["OPENAI_API_KEY"] = "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"

    memory_saver = MemorySaver()
    agent = create_react_agent("openai:gpt-4.1", tools, checkpointer=memory_saver)
    return agent


async def chat_loop():
    """
    The main chat loop that feeds human messages to chatbot and display responses
    """


    # OpenAI API key
    import os
    os.environ["OPENAI_API_KEY"] = "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"

    # Set things up
    tools = await connect_to_servers()
    agent = setup_agent_w_memory(tools)
    config = {"configurable": {"thread_id" : "1"}}

    # # A test sample
    # math_response = await agent.ainvoke({"messages": "what's (3 + 5) x 12?"}, config=config)
    # pretty_print(math_response['messages'])
    # math_response = await agent.ainvoke({"messages": "what's previous result?"}, config=config)
    # pretty_print(math_response['messages'])
    # pretty_print(agent.get_state(config).values['messages'])

    # Main chat loop
    # # Custom response printing
    # response = await agent.ainvoke({"messages": user_input}, config=config)
    # pretty_print(response['messages'])

    s = socket.socket()
    print("Socket successfully created")
    port = 12345
    s.bind(('', port))
    print("Socket binded to %s" % (port))
    s.listen(3)
    print("Socket is listening")

    while True:
        conn, addr = s.accept()
        print('Got connection from', addr)
        user_input = conn.recv(8192).decode()


        response = await agent.ainvoke({"messages": user_input}, config=config)
        conn.send(pretty_print(response['messages']).encode())

        conn.close()



if __name__ == '__main__':
    asyncio.run(chat_loop())

