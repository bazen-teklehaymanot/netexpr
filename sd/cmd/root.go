package cmd

import (
	"encoding/base64"
	"fmt"
	"os"

	"github.com/spf13/cobra"
	"google.golang.org/protobuf/encoding/protojson"
	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/reflect/protodesc"
	"google.golang.org/protobuf/reflect/protoreflect"
	"google.golang.org/protobuf/types/descriptorpb"
	"google.golang.org/protobuf/types/dynamicpb"
)

var (
	// base64 encoded payload
	payload string
	// path to proto-buf descriptor
	descriptor string
	// name of the message
	name string
	// struct name
	sname string
)

var rootCmd = &cobra.Command{
	Use:   "sd",
	Short: "Serializes and deserializes JSON and ProtoBuf",
	Long:  `Serializes and deserializes JSON and ProtoBuf`,
}

func Execute() {
	err := rootCmd.Execute()
	if err != nil {
		os.Exit(1)
	}
}

func init() {
	rootCmd.Flags().BoolP("toggle", "t", false, "Help message for toggle")
}

func terminate(err error) {
	if err != nil {
		os.Stderr.WriteString(err.Error())
		os.Exit(1)
	}
}

func compileDescriptor(descriptor string, name string, fp string) (protoreflect.MessageDescriptor, error) {
	descriptorSet := descriptorpb.FileDescriptorSet{}
	dbytes, err := base64.StdEncoding.DecodeString(descriptor)
	if err != nil {
		return nil, err
	}
	err = proto.Unmarshal(dbytes, &descriptorSet)
	if err != nil {
		return nil, err
	}

	localRegistry, err := protodesc.NewFiles(&descriptorSet)
	if err != nil {
		return nil, err
	}

	filePath := fmt.Sprintf("%v.proto", fp)
	fileDesc, err := localRegistry.FindFileByPath(filePath)
	if err != nil {
		return nil, err
	}

	msgsDesc := fileDesc.Messages()
	msgDesc := msgsDesc.ByName(protoreflect.Name(name))

	return msgDesc, nil
}

func protoToJson(m []byte, desc protoreflect.MessageDescriptor) ([]byte, error) {
	newMsg := dynamicpb.NewMessage(desc)
	err := proto.Unmarshal(m, newMsg)
	if err != nil {
		return nil, err
	}

	jsonBytes, err := protojson.Marshal(newMsg)
	if err != nil {
		return nil, err
	}

	return jsonBytes, nil
}

func jsonToProto(msgBytes []byte, desc protoreflect.MessageDescriptor) ([]byte, error) {
	newMsg := dynamicpb.NewMessage(desc)
	err := protojson.Unmarshal(msgBytes, newMsg)
	if err != nil {
		return nil, err
	}

	protoBytes, err := proto.Marshal(newMsg)
	if err != nil {
		return nil, err
	}

	return protoBytes, nil
}
