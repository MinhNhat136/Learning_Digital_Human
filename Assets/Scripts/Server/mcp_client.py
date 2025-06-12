
import os
import asyncio

from langchain_mcp_adapters.client import MultiServerMCPClient
from langchain_openai import ChatOpenAI
from langgraph.prebuilt import create_react_agent
from langgraph.checkpoint.memory import MemorySaver
from langchain.schema.messages import HumanMessage, AIMessage, ToolMessage
import nbimporter
import argparse


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
    for msg in messages:
        if isinstance(msg, HumanMessage):
            print(f"{msg_header("Human")}{msg.content}")

        elif isinstance(msg, AIMessage):
            if msg.tool_calls:
                for tool_call in msg.tool_calls:
                    print(f"{msg_header("AI to Tool")}{tool_call['name']}({tool_call['args']})")
            if msg.content:
                print(f"{msg_header("AI")}{msg.content}")

        elif isinstance(msg, ToolMessage):
            print(f"{msg_header("Tool")}{msg.name} → {msg.content}")

    print(msg_header(text='END RESPONSE', delimiter='*'))

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
    llm_qwen3 = ChatOpenAI(
        api_key="ollama",
        model='legal-qwen3',
        base_url="http://localhost:11434/v1",
    )

    """
    Create ReAct agent with memory
    """
    memory_saver = MemorySaver()
    agent = create_react_agent(llm_qwen3, tools, checkpointer=memory_saver)
    return agent

async def chat_loop(user_input):
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
    with open("result_llm.txt", "w", encoding='utf-8') as f:
        async for step in agent.astream(
            {"messages": user_input},
            config=config,
            stream_mode="values",
        ):
            f.write(f"{step["messages"][-1].content}\n")


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("user_input", type=str)
    args = parser.parse_args()
    asyncio.run(chat_loop(args.user_input))

