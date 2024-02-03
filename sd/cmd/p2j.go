package cmd

import (
	"context"
	"encoding/base64"
	"os"

	"github.com/spf13/cobra"
)

var p2jCmd = &cobra.Command{
	Use:   "p2j --payload=payload --desc=desc --name=name --sn=sn",
	Short: "ProtoBuf to JSON",
	Long:  `Converts ProtoBuf to JSON`,
	Run: func(cmd *cobra.Command, args []string) {
		handleP2J(cmd.Context())
	},
}

func init() {
	p2jCmd.Flags().StringVar(&payload, "payload", "", "proto-buf payload")
	p2jCmd.Flags().StringVar(&descriptor, "desc", "", "proto-buf descriptor")
	p2jCmd.Flags().StringVar(&name, "name", "", "name of the message")
	p2jCmd.Flags().StringVar(&sname, "sn", "", "strut name")
	p2jCmd.MarkFlagRequired("payload")
	p2jCmd.MarkFlagRequired("desc")
	p2jCmd.MarkFlagRequired("name")
	p2jCmd.MarkFlagRequired("sn")
	rootCmd.AddCommand(p2jCmd)
}

func handleP2J(context context.Context) {
	pb, err := base64.StdEncoding.DecodeString(payload)
	if err != nil {
		terminate(err)
	}
	desc, err := compileDescriptor(descriptor, name, sname)
	if err != nil {
		terminate(err)
	}
	jsonBytes, err := protoToJson(pb, desc)
	if err != nil {
		terminate(err)
	}
	j64 := base64.StdEncoding.EncodeToString(jsonBytes)
	os.Stdout.WriteString(j64)
}
